'use strict';
define(['jquery', 'knockout'], function ($, ko) {
    ko.bindingHandlers.hasFocus = {
        init: function (element, valueAccessor) {
            var $el = $(element);
            if ($el.prop('tabindex') < 0) { // if there's no tabindex, this will be -1
                $el.prop('tabindex', -1);   // explicitly set a tabindex so that this binding (i.e. the focusin and focusout handlers) can be used with non input/textarea elements likes divs
            }
            $el.focusin(function () {
                var value = valueAccessor();
                value(true);
            });
            $el.focusout(function () {
                var value = valueAccessor();
                value(false);
            });
        },
        update: function (element, valueAccessor) {
            var value = valueAccessor();
            if (ko.unwrap(value))
                element.focus();
            else
                element.blur();
        }
    };
});