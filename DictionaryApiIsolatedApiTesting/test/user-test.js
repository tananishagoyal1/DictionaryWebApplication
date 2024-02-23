const SwaggerParser = require('swagger-parser')
var parser = new SwaggerParser()
const hippie = require('hippie-swagger');
const chai = require('chai');
const expect = chai.expect;

const jwtHelper = require('../jwt-generator.js')
const baseUrl = "http://localhost:8030";

let dereferencedSwagger;

let hippieOptions = {
    validateResponseSchema: false,
    validateParameterSchema: false,
    errorOnExtraParameters: false,
    errorOnExtraHeaderParameters: false
};

describe('User Tests', function () {
    this.timeout(10000);
    before(function (done) {

        parser.dereference(baseUrl + '/swagger/v1/swagger.json', function (err, api) {
            if (err) return done(err)
            dereferencedSwagger = api;
            done();
        });
    });

    describe('Login Tests', function () {

        it('After Successful login with return 200 response', function (done) {
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('User-Agent', 'hippine')
                .json()
                .post('/api/User/Login')
                .send({
                    email: 'TestId@gmail.com',
                    password: 'Test@1234'
                })
                .end(function (err, res, body) {
                    if (err) return done(err);
                    //assert
                    const token = body.jwtToken;
                    expect(res.statusCode).to.equal(200);
                    expect(body).to.have.property('jwtToken');
                    expect(token).to.not.be.null;

                    done();
                });

        });


        it('After an unsuccessful login', function (done) {
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('User-Agent', 'hippine')
                .json()
                .post('/api/User/Login')
                .send({
                    email: 'Test13@gmail.com',
                    password: 'Test@1234'
                })
                .end(function (err, res, body) {
                    if (err) return done(err);

                    //assert
                    expect(res.statusCode).to.equal(401);
                    done();
                });

        });

        it('When user doesnot enter Email Field', function (done) {
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('User-Agent', 'hippine')
                .json()
                .post('/api/User/Login')
                .send({
                    email: '',
                    password: 'Test@1234'
                })
                .end(function (err, res, body) {
                    if (err) return done(err);

                    //assert
                    expect(body.errors.Email[0]).to.equal("The Email field is required.");
                    expect(body.status).to.equal(400);
                    done();
                });

        });

        it('When user doesnot enter Password Field', function (done) {
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('User-Agent', 'hippine')
                .json()
                .post('/api/User/Login')
                .send({
                    email: 'TestId@gmail.com',
                    password: ''
                })
                .end(function (err, res, body) {
                    if (err) return done(err);

                    //assert
                    expect(body.errors.Password[0]).to.equal("The Password field is required.");
                    expect(body.status).to.equal(400);
                    done();
                });

        });

    });

    describe('Register Endpoint Tests', function () {

        it('After Successful Register  with return 201 response', function (done) {
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('User-Agent', 'hippine')
                .json()
                .post('/api/User/Register')
                .send({
                    Firstname: "Rahul",
                    email: 'Rahul112@gmail.com',
                    password: 'Rahul@1234'
                })
                .end(function (err, res, body) {
                    if (err) return done(err);
                    //assert
                    expect(res.statusCode).to.equal(201);
                    done();
                });

        });

        it('After an unsuccessful Register', function (done) {
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('User-Agent', 'hippine')
                .json()
                .post('/api/User/Register')
                .send({
                    FirstName: 'NewTest',
                    email: 'TestId@gmail.com',
                    password: 'Test@1234'
                })
                .end(function (err, res, body) {
                    if (err) return done(err);

                    //assert
                    expect(res.statusCode).to.equal(409);
                    done();
                });

        });


        it('When user enters an Invalid Email format', function (done) {
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('User-Agent', 'hippine')
                .json()
                .post('/api/User/Register')
                .send({
                    FirstName: 'NewTest',
                    email: 'TestIdgmailcom',
                    password: 'Test@1234'
                })
                .end(function (err, res, body) {
                    if (err) return done(err);

                    //assert
                    expect(body.errors.Email[0]).to.equal("Invalid Email Format.");
                    expect(body.status).to.equal(400);
                    done();
                });

        });

        it('When user enters an Invalid Password format', function (done) {
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('User-Agent', 'hippine')
                .json()
                .post('/api/User/Register')
                .send({
                    FirstName: 'NewTest',
                    email: 'TestId@gmail.com',
                    password: 'Test24'
                })
                .end(function (err, res, body) {
                    if (err) return done(err);

                    //assert
                    expect(body.errors.Password[0]).to.equal("Password must contain Capital, small, number and special Characters with minimum length 8.");
                    expect(body.status).to.equal(400);
                    done();
                });

        });

        it('When user does not enter FirstName', function (done) {
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('User-Agent', 'hippine')
                .json()
                .post('/api/User/Register')
                .send({
                    FirstName: '',
                    email: 'TestId@gmail.com',
                    password: 'Test@1324'
                })
                .end(function (err, res, body) {
                    if (err) return done(err);

                    //assert
                    expect(body.errors.FirstName[0]).to.equal("The FirstName field is required.");
                    expect(body.status).to.equal(400);
                    done();
                });

        });

        it('When user does not enter Email  ', function (done) {
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('User-Agent', 'hippine')
                .json()
                .post('/api/User/Register')
                .send({
                    FirstName: 'NewTest',
                    email: '',
                    password: 'Test@1324'
                })
                .end(function (err, res, body) {
                    if (err) return done(err);

                    //assert
                    expect(body.errors.Email[0]).to.equal("The Email field is required.");
                    expect(body.status).to.equal(400);
                    done();
                });

        });


    });

    describe('User History Test', function () {

        it('Searching for user history', function (done) {
            const testJWT = jwtHelper.generateJWT();
            hippie(dereferencedSwagger, hippieOptions)

                .base(baseUrl)
                .header('Authorization', `Bearer ${testJWT}`)
                .json()
                .get('/api/User/History')
                .end(function (err, res, body) {
                    if (err) return done(err);
                    //assert
                    expect(res.statusCode).to.equal(200);
                    done();
                });

        });

        it('Searching for user history without Jwt', function (done) {
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .json()
                .get('/api/User/History')
                .end(function (err, res, body) {
                    if (err) return done(err);
                    //assert
                    expect(res.statusCode).to.equal(401);
                    done();
                });

        });

        it('Checking for Word Count for a particular User ', function (done) {
          const testJWT = jwtHelper.generateJWT();
          hippie(dereferencedSwagger, hippieOptions)

              .base(baseUrl)
              .header('Authorization', `Bearer ${testJWT}`)
              .json()
              .get('/api/User/History')
              .end(function (err, res, body) {
                  if (err) return done(err);
                  //assert
                  expect(res.statusCode).to.equal(200);
                  expect(body).to.have.length.greaterThan(0);
                  done();
              });

      });
    });

});

