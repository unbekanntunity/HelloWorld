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

    return await sendRequest(urlWithQuery, request);
}

export async function sendFORMRequest(method, url, body = undefined, token = undefined, query = undefined) {
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
        request.body = body;
    }

    if (token) {
        request.headers['Authorization'] = `Bearer ${token}`;
    }

    return await sendRequest(urlWithQuery, request);
}

async function sendRequest(urlWithQuery, request) {
    const response = await fetch(domainName + urlWithQuery, request);

    //console.log(response);

    if (response.status === 204)
        return;

    try {
        return response.json();
    } catch (e) {
       console.log(response);
    }
}

export function handleUpdateRating(id, type, token, onError, onSuccess) {
    sendJSONRequest("PATCH", `/${type}/update_rating/${id}`, undefined, token)
        .then(response => {
            onSuccess(response);
    }, error => {
        onError(error.message);
    });
}