const domainName = "https://localhost:7113/api/v1";

export async function sendJSONRequest(method, url, body = undefined, token = undefined, query = undefined) {
    let urlWithQuery = url;

    if (query) {
        const queryParams = Object.entries(query);

        if (queryParams.length > 0) {
            urlWithQuery += `?${queryParams[0][0]}=${queryParams[0][1]}`;

            for (let i = 1; i < query.entries; i++) {
                urlWithQuery += `&${queryParams[i][0]}=${queryParams[i][1]}`
            }
        }
    }
    let request = {
        method,
        headers: {
        },
    };

    if (body) {
        request.headers['Content-Type'] = "application/json";
        request.body = JSON.stringify(body);
    }

    if (token) {
        request.headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(domainName + urlWithQuery, request);
    return response.json();
}

export async function sendFORMRequest(method, url, body = undefined, token = undefined, query = undefined) {
    let urlWithQuery = url;

    if (query && query.keys.lenght > 0) {
        urlWithQuery += "?";
        for (let i = 0; i < query.keys.length; i++) {
            urlWithQuery += `$ ${query.keys[i]}`
        }
    }

    let request = {
        method,
        headers: {
        },
    };

    if (body) {
        request.body = body;
    }

    if (token) {
        request.headers['Authorization'] = `Bearer ${token}`;
    }

    try {
        const response = await fetch(domainName + urlWithQuery, request);
        return response.json();

    } catch (e) {
        console.log(e);
    }
}


