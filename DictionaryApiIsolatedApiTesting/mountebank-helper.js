
const url = "http://localhost:2525/imposters";
const port = 8090

function postImposter(body) {

    return fetch(url, {
                    method:'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(body)
                });
}


function createImposter()
{
    const imposter ={
        port:port,
        protocol:'http',
        stubs:[],
    }
    return imposter
}

function updateImposter(stubs)
{
    const updateUrl = `${url}/${port}/stubs`
    return fetch(updateUrl, {
        method:'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(stubs)
    });
    
}
module.exports= { postImposter ,createImposter, updateImposter};