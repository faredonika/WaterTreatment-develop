'use strict';

define(['jquery', 'knockout', 'pager', 'webService', 'App/states'], function ($, ko, pager, webService, states) {
    var vm = function () {
        var self = this;

        self.vendors = ko.observableArray();
        self.states = ko.observable(states);

        self.filters = {
            Name: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            State: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' }})
        };

        var search = function (payload) {
            return webService.Post('/User/SearchVendors/', payload);
        };

        self.pager = new pager(search, self.filters, self.vendors);
        self.pager.sortDirection(false);
        self.pager.sortBy('Name');

        self.getFullStateName = function (abbreviation) {
            var match = $.grep(self.states(), function (state) {
                return state.abbreviation == abbreviation;
            })[0];

            return match.name;
        }
    };

    return vm;
})