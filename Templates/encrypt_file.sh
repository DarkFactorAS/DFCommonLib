#!/usr/bin/env bash
set -euo pipefail

read -r -s -p "Enter encryption key: " ENCRYPTION_KEY
echo

if [[ -z "$ENCRYPTION_KEY" ]]; then
	echo "Encryption key is required." >&2
	exit 1
fi

echo "encryption key: $ENCRYPTION_KEY"

dotnet run --project "../DFCommonLib.ConfigEncryptor/DFCommonLib.ConfigEncryptor.csproj" -- $ENCRYPTION_KEY --file appsettings.cleartext.json --out appsettings.encrypted.out.json