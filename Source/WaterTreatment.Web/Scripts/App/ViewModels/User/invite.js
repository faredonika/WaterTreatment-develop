define(['knockout'], function (ko) {

    return function () {

        var self = this;

        self.password = ko.observable().extend({ required: true });
        self.passwordCheck = ko.observable().extend({ required: true });

        self.password.extend({
            validation: {
                validator: function (val) {
                    return val === self.passwordCheck();
                },
                message: 'Must Match'
            }
        });

        self.passwordCheck.extend({
            validation: {
                validator: function (val) {
                    return val === self.password();
                },
                message: 'Must Match'
            }
        });

        self.canSave = ko.computed(function () {

            var errors = ko.validation.group([self.password, self.passwordCheck]);

            var errorFree = errors().length === 0;

            return self.password() === self.passwordCheck() && errorFree;
        }, self);

    };

});