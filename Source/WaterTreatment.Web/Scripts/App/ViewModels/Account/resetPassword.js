'use strict';

define(['knockout'], function (ko) {
    var viewModel = function (initialState) {
        var self = this;

        self.email = ko.observable(initialState.Email);
        self.password = ko.observable();
        self.passConfirm = ko.observable();

        self.email.extend({
            required: true,
            email: true
        });

        self.password.extend({ required: true });

        self.passConfirm.extend({
            validation: {
                validator: function (val) {
                    return val === self.password();
                },
                message: 'Passwords must match'
            }
        })

        self.canSubmit = ko.computed(function () {
            var errors = ko.validation.group([self.email, self.password, self.passConfirm]);

            return errors().length === 0;
        });
    };

    return viewModel;
});