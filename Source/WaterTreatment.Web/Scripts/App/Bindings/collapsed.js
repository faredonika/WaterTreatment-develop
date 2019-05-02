'use strict';
define(['jquery', 'knockout', 'bootstrap'], function ($, ko) {
    ko.bindingHandlers['collapsed'] = {
        'init': function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor();
            var $el = $(element);
            value($el.hasClass('collapse'));
            $el.on('hidden.bs.collapse', function () {
                value(true);
            });
            $el.on('shown.bs.collapse', function () {
                value(false);
            });
        },
        'update': function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor();
            var $el = $(element);
            $el.collapse(value() === true ? 'hide' : 'show');
        }
    };
});