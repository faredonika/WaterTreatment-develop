define(['jquery', 'q'], function($, Q) {

    var appendSecurityToken = function (payload) {
        payload.__RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    };

    var WebService = function() {

        var self = this;

        self.Get = function (url, payload) {
            /// <summary>Performs an AJAX GET request expecting a JSON result.</summary>
            /// <param name="url" type="string"></param>
            /// <param name="payload" type="object">An object that will be flattened into GET parameters.</param>
            /// <returns type="promise">Returns a Q promise.</returns>
            var deferred = Q.defer();
            $.ajax({
                type: 'GET',
                url: url,
                data: payload,
                cache: false,
                dataType: 'json',
                error: deferred.reject,
                success: deferred.resolve,
                timeout: 300000
            });
            return deferred.promise;
        };

        self.Post = function (url, payload, headers, dataType) {
            /// <summary>Performs an AJAX POST request expecting a JSON result.</summary>
            /// <param name="url" type="string"></param>
            /// <param name="payload" type="object">An object that will be serialized to JSON and sent in a POST request.</param>
            /// <returns type="promise">Returns a Q promise.</returns>
            var deferred = Q.defer();
            $.ajax({
                type: 'POST',
                url: url,
                data: JSON.stringify(payload),
                contentType: 'application/json',
                processData: false,
                cache: false,
                dataType: dataType || 'json',
                headers: headers || {},
                error: deferred.reject,
                success: deferred.resolve,
                timeout: 300000
            });
            return deferred.promise;
        };

        self.PostCSRF = function(url, payload, headers, dataType) {
            appendSecurityToken(headers || {});
            return self.Post(url, payload, headers, dataType);
        };
        
        self.PostFile = function (url, payload) {
            var deferred = Q.defer();
            $.ajax({
                type: 'POST',
                url: url,
                data: payload,
                cache: false,
                processData: false, // Don't process the files
                contentType: false, // Set content type to false as jQuery will tell the server its a query string request
                error: deferred.reject,
                success: deferred.resolve
            });
            return deferred.promise;
        };

        self.BuildElements = function (form, element, name) {
            
            for (var key in element) {

                if (element.hasOwnProperty(key)) {

                    var effectiveName = name + key;

                    if (element[key] instanceof Array) {

                        element[key].forEach(function (innerElement, index) {

                            self.BuildElements(form, innerElement, effectiveName + '[' + index + '].');

                        });

                    }

                    else {
                        var hiddenField = document.createElement("input");

                        hiddenField.setAttribute("type", "hidden");
                        hiddenField.setAttribute("name", effectiveName);
                        hiddenField.setAttribute("value", element[key]);

                        form.appendChild(hiddenField);
                    }
                }
            }
        };

        self.SubmitForm = function (path, params, method) {
            /// <summary>Builds a form dynamically through the JSON payload</summary>
            method = method || "post";

            var form = document.createElement("form");
            form.setAttribute("method", method);
            form.setAttribute("action", path);

            this.BuildElements(form, params, '', '');

            document.body.appendChild(form);
            form.submit();
        };
        

        self.SubmitFormCSRF = function (path, params, method) {
            appendSecurityToken(params);
            self.SubmitForm(path, params, method);
        };

    };

    return new WebService();
})