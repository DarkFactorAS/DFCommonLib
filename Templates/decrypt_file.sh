#!/usr/bin/env bash
set -euo pipefail

read -r -s -p "Enter encryption key: " ENCRYPTION_KEY
echo

if [[ -z "$ENCRYPTION_KEY" ]]; then
	echo "Encryption key is required." >&2
	exit 1
fi

echo "encryption key: $ENCRYPTION_KEY"
dotnet run --project "../DFCommonLib.ConfigDecryptor/DFCommonLib.ConfigDecryptor.csproj" -- $ENCRYPTION_KEY --file appsettings.encrypted.json --out appsettings.cleartext.out.json