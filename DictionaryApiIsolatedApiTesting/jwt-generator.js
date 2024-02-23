const jwt = require('jsonwebtoken');

const secretKey = "3e0a5c6b8b0f11e0a1bbf0fdd8b74eefc5d30f7bbf1a971b63cdae274adb570b"
const payload = {
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "NewTest",
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "abcde",

    "iss": "http://dictionaryapi",
    "aud": "http://dictionaryapi"
};


function generateJWT() {

    const token = jwt.sign(payload, secretKey, { expiresIn: '1h', noTimestamp: true });
    return token;
}
module.exports = {
    generateJWT
};