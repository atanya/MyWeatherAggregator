window.weatherAggregator = {};

weatherAggregator.proxy = function () {

    var sendAjaxRequest = function(type, url, data, onSuccess, onComplete) {

        var jsonRequest = JSON.stringify(data);

        $.ajax({
            url: url,
            type: type,
            data: jsonRequest,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: onSuccess,
            complete: onComplete,
            error: function (xhr) {
                alert(xhr.status);
            }
        });
    };

    return {
        getSources: function (onSuccess, onComplete) {
            sendAjaxRequest("GET", "/api/Weather/Sources", null, onSuccess, onComplete);
        },

        getWeather: function (data, onSuccess, onComplete) {
            sendAjaxRequest("POST", "/api/Weather/GetWeather", data, onSuccess, onComplete);
        },

        sendFeedback: function(data) {
            sendAjaxRequest("POST", "/api/Feedback/Add", data);
        },

        subscribe: function (data) {
            sendAjaxRequest("POST", "/api/Subscription/Subscribe", data);
        }
    };
}();