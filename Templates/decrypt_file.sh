#!/usr/bin/env bash
set -euo pipefail

: "${ENCRYPTION_KEY:?ENCRYPTION_KEY environment variable is required}"
INPUT_FILE="${1:-appsettings.encrypted.json}"

dotnet run --project "../DFCommonLib.ConfigDecryptor/DFCommonLib.ConfigDecryptor.csproj" -- "$ENCRYPTION_KEY" --file "$INPUT_FILE" --out