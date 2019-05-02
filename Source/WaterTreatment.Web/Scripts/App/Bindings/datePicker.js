define(['jquery', 'knockout', 'bootstrap', 'moment', 'bootstrap-datepicker'], function ($, ko) {
    ko.bindingHandlers.datepicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            //initialize datepicker with some optional options
            var options = allBindingsAccessor().datepickerOptions || {},
                $el = $(element);

            $el.datepicker(options);

            //handle the field changing by registering datepicker's changeDate event
            ko.utils.registerEventHandler(element, "changeDate", function () {
                var observable = valueAccessor();
                observable($el.datepicker("getDate"));
            });

            //clearing the date does not fire the changeDate event, so handle it explicitly
            ko.utils.registerEventHandler(element, "clearDate", function () {
                var observable = valueAccessor();
                observable(undefined);
            });

            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $el.datepicker("destroy");
            });

        },
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor()),
                $el = $(element);

            //handle date data coming via json from Microsoft
            if (String(value).indexOf('/Date(') == 0) {
                value = new Date(parseInt(value.replace(/\/Date\((.*?)\)\//gi, "$1")));
            }

            var current = $el.datepicker("getDate");

            if (value - current !== 0) {
                $el.datepicker("setDate", value);
            }
        }
    };
});