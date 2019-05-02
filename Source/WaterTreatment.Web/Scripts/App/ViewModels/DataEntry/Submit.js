define(['knockout', 'jquery'], function (ko, $) {

    return function (reportId) {

        var self = this;
        self.isProcessingRequest = ko.observable(false);

        self.edit = function () {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            window.location.href = '/DataEntry/Edit/' + reportId;
        };

        self.submit = function () {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            if (confirm('Are you sure that you want to submit this report?')) {
                $("#submitForm").submit();
            } else {
                self.isProcessingRequest(false);
            }
        }

    };

});