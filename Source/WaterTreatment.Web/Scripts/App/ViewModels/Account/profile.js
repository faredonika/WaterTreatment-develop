'use strict';

define(['knockout', 'bootstrap', 'koVal'], function (ko) {
    var viewModel = function (initialState) {
        var self = this;

        self.name = ko.observable();
        self.firstName = ko.observable();
        self.lastName = ko.observable();
        self.email = ko.observable();

        if (initialState) {
            self.name(initialState.Username);
            self.firstName(initialState.FirstName);
            self.lastName(initialState.LastName);
            self.email(initialState.Email);
        }

        self.name.extend({ required: true }).extend({
            validation: {
                validator: function (val) {
                    return !/\W/g.test(val);
                },
                message: 'Username can only contain alphanumeric characters.'
            }
        });
        self.email.extend({
            required: true,
            email: true
        });

        self.canSave = ko.computed(function () {
            var errors = ko.validation.group([self.name, self.email]);

            return errors().length === 0;
        });
    };

    return viewModel;
});