DFCommonLib is a generic utility library used in other projects

Version Change:
* 1.7.2 : Refactored encryption to explicit key input across DFCrypt/config, added AppName-scoped env key resolution (<AppName>_EncryptionKey), enhanced ConfigEncryptor file mode (--file, optional --out, encrypted- prefixed output, IsConfigEncrypted=true, full JSON console output), updated related tests/encrypted config, and bumped DFCommonLib plus TestApp client/server versions to 1.7.2.
* 1.7.0 : Updated DFCrypt encryption to AES-GCM (authenticated encryption), replaced prior AES-CBC flow, and added explicit encryption key configuration via DFCommonLib_EncryptionKey
* 1.5.9 : Framework for OAuth2 client and server