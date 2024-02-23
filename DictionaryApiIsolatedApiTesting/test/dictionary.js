const SwaggerParser = require('swagger-parser')
var parser = new SwaggerParser()
const hippie = require('hippie-swagger');
const chai = require('chai');
const expect = chai.expect;
const jwtHelper = require('../jwt-generator.js')
const {addStubForCorrectWord,addStubForIncorrectWord} = require('../dictionary-service.js')
const {createImposter,postImposter } = require("../mountebank-helper.js")
const baseUrl = "http://localhost:8030";


let dereferencedSwagger;

let hippieOptions = {
    validateResponseSchema: false,
    validateParameterSchema: false,
    errorOnExtraParameters: false,
    errorOnExtraHeaderParameters: false
};

describe('Dictionary Tests', function () {
    this.timeout(10000);
    before(function (done) {
       const imposter = createImposter();
       postImposter(imposter);
        parser.dereference(baseUrl + '/swagger/v1/swagger.json', function (err, api) {
            if (err) return done(err)
            dereferencedSwagger = api;
            done();
        });
    });

    describe('Search a word in Dictionary', function () {

        it('After Searching Correct Word and sending correct JWT', function (done) {
            const testJWT = jwtHelper.generateJWT();
            addStubForCorrectWord('test');
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('Authorization', `Bearer ${testJWT}`)
                .json()
                .get('/api/Dictionary/Search')
                .qs({ word: 'test' })
                .end(function (err, res, body) {
                    if (err) return done(err);
                    //assert
                    expect(res.statusCode).to.equal(200);
                    done();
                });

        });

        it('After Searching Correct Word without JWT', function (done) {

            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .json()
                .get('/api/Dictionary/Search')
                .qs({ word: 'test' })
                .end(function (err, res, body) {
                    if (err) return done(err);
                    //assert
                    expect(res.statusCode).to.equal(401);
                    done();
                });

        });

        it('After Searching Incorrect Word and sending correct JWT', function (done) {
            const testJWT = jwtHelper.generateJWT();
            addStubForIncorrectWord('dfghjkl')
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('Authorization', `Bearer ${testJWT}`)
                .json()
                .get('/api/Dictionary/Search')
                .qs({ word: 'dfghjkl' })
                .end(function (err, res, body) {
                    if (err) return done(err);
                    //assert
                    expect(res.statusCode).to.equal(404);
                    done();
                });

        });


        it('When user does not enter Word', function (done) {
            const testJWT = jwtHelper.generateJWT();
            hippie(dereferencedSwagger, hippieOptions)
                .base(baseUrl)
                .header('Authorization', `Bearer ${testJWT}`)
                .json()
                .get('/api/Dictionary/Search')
                .qs({ word: "" })
                .end(function (err, res, body) {
                    if (err) return done(err);

                    //assert
                    expect(body.errors.word[0]).to.equal("The word field is required.");
                    expect(body.status).to.equal(400);
                    done();
                });

        });

    });

});

