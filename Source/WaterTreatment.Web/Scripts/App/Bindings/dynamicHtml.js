'use strict';
define(['jquery', 'knockout'], function ($, ko) {
    ko.bindingHandlers['dynamicHtml'] = {
        'init': function () {
            return { 'controlsDescendantBindings': true };
        },
        'update': function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            ko.utils.setHtml(element, valueAccessor());
            ko.applyBindingsToDescendants(bindingContext, element);
        }
    };
});