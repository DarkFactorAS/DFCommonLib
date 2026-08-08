DFCommonLib is a generic utility library used in other projects

Version Change:
* 1.7.0 : Updated DFCrypt encryption to AES-GCM (authenticated encryption), replaced prior AES-CBC flow, and now requires passing the encryption key explicitly (config decryption resolves key from <AppName>__EncryptionKey)
* 1.5.9 : Framework for OAuth2 client and server