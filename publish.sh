#!/usr/bin/env bash
set -euo pipefail

# Usage: ./publish.sh [patch|minor|major]
BUMP_KIND="${1:-patch}"

ROOT_DIR="$(cd "$(dirname "$0")" && pwd)"
ARTIFACTS_DIR="${ROOT_DIR}/artifacts"
PACK_PROJECT="${ROOT_DIR}/src/CuriousInc.Common.Functional/CuriousInc.Common.Functional.csproj"
SOURCE_GENERATOR_PROJECT="${ROOT_DIR}/src/CuriousInc.Utilities.Source.Generators/CuriousInc.Utilities.Source.Generators.csproj"

command -v dotnet >/dev/null || { echo "dotnet CLI is required"; exit 1; }
: "${NUGET_API_KEY:?Set NUGET_API_KEY before running this script}"

echo "==> Cleaning artifacts directory"
rm -rf "${ARTIFACTS_DIR}"
mkdir -p "${ARTIFACTS_DIR}"

build_project() {
 local project="$1"
 echo "==> Building ${project}"
 dotnet restore "${project}"
 dotnet build "${project}" -c Release
}

extract_version() {
 local file="$1"
 awk '
   BEGIN { in_comment = 0 }
   {
     line = $0

     if (in_comment) {
       if (line ~ /-->/) {
         sub(/^.*-->/, "", line)
         in_comment = 0
       } else {
         next
       }
     }

     while (line ~ /<!--/) {
       prefix = substr(line, 1, RSTART - 1)
       comment_rest = substr(line, RSTART + RLENGTH)

       if (comment_rest ~ /-->/) {
         suffix = substr(comment_rest, index(comment_rest, "-->") + 3)
         line = prefix suffix
       } else {
         line = prefix
         in_comment = 1
         break
       }
     }

     if (match(line, /<Version>[^<]+<\/Version>/)) {
       version = substr(line, RSTART, RLENGTH)
       gsub(/^<Version>|<\/Version>$/, "", version)
       print version
       exit
     }
   }
 ' "${file}"
}

bump_version() {
 local version="$1" kind="$2"
 local major=0 minor=0 patch=0
 IFS='.' read -r major minor patch <<<"${version}"
 minor=${minor:-0}
 patch=${patch:-0}

 case "${kind}" in
   major) major=$((major + 1)); minor=0; patch=0 ;;
   minor) minor=$((minor + 1)); patch=0 ;;
   patch) patch=$((patch + 1)) ;;
   *) echo "Unsupported bump kind: ${kind}" >&2; exit 1 ;;
 esac

 echo "${major}.${minor}.${patch}"
}

write_version() {
 local file="$1" new_version="$2" tmp
 tmp="$(mktemp)"
 awk -v new_version="${new_version}" '
   BEGIN { in_comment = 0 }
   {
     line = $0

     if (!done && !in_comment) {
       candidate = line

       while (candidate ~ /<!--/) {
         prefix = substr(candidate, 1, RSTART - 1)
         comment_rest = substr(candidate, RSTART + RLENGTH)

         if (comment_rest ~ /-->/) {
           suffix = substr(comment_rest, index(comment_rest, "-->") + 3)
           candidate = prefix suffix
         } else {
           candidate = prefix
           break
         }
       }

       if (match(candidate, /<Version>[^<]+<\/Version>/)) {
         sub(/<Version>[^<]+<\/Version>/, "<Version>" new_version "</Version>", line)
         done = 1
       }
     }

     print line

     if (in_comment) {
       if ($0 ~ /-->/) {
         in_comment = 0
       }
     } else if ($0 ~ /<!--/ && $0 !~ /-->/) {
       in_comment = 1
     }
   }
 ' "${file}" > "${tmp}"
 mv "${tmp}" "${file}"
}

echo $ROOT_DIR

echo "==> Building projects"
build_project "${PACK_PROJECT}"
build_project "${SOURCE_GENERATOR_PROJECT}"

echo "==> Incrementing version (${BUMP_KIND}) in ${PACK_PROJECT}"
CURRENT_VERSION="$(extract_version "${PACK_PROJECT}")"
NEW_VERSION="$(bump_version "${CURRENT_VERSION}" "${BUMP_KIND}")"
write_version "${PACK_PROJECT}" "${NEW_VERSION}"
echo "==> New version: ${NEW_VERSION}"

echo "==> Packing ${PACK_PROJECT}"
dotnet pack "${PACK_PROJECT}" -c Release -o "${ARTIFACTS_DIR}" /p:ContinuousIntegrationBuild=true

echo "==> Incrementing version (${BUMP_KIND}) in ${SOURCE_GENERATOR_PROJECT}"
SOURCE_GEN_VERSION="$(extract_version "${SOURCE_GENERATOR_PROJECT}")"
SOURCE_GEN_NEW_VERSION="$(bump_version "${SOURCE_GEN_VERSION}" "${BUMP_KIND}")"
write_version "${SOURCE_GENERATOR_PROJECT}" "${SOURCE_GEN_NEW_VERSION}"
echo "==> New version: ${SOURCE_GEN_NEW_VERSION}"

echo "==> Packing ${SOURCE_GENERATOR_PROJECT}"
dotnet pack "${SOURCE_GENERATOR_PROJECT}" -c Release -o "${ARTIFACTS_DIR}" /p:ContinuousIntegrationBuild=true

echo "==> Publishing packages to NuGet"
shopt -s nullglob
for pkg in "${ARTIFACTS_DIR}"/*.nupkg; do
 [[ "${pkg}" == *.snupkg ]] && continue
 echo "----> Pushing $(basename "${pkg}")"
 dotnet nuget push "${pkg}" \
   --api-key "${NUGET_API_KEY}" \
   --source "https://api.nuget.org/v3/index.json" \
   --skip-duplicate
done
shopt -u nullglob

echo "==> Done (version ${NEW_VERSION}) (version ${SOURCE_GEN_NEW_VERSION})"
