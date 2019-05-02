define(['jquery', 'knockout'], function ($, ko) {
    // Note remove the data-toggle attribute from your bootstrap markup as it
    // interferes with the Knockout checked binding. See: http://stackoverflow.com/questions/30317032/
    ko.bindingHandlers.bsChecked = {
        init: function (element, valueAccessor, allBindingsAccessor,
        viewModel, bindingContext) {
            var value = valueAccessor();
            var newValueAccessor = function () {
                return {
                    change: function () {
                        value(element.value);
                    }
                }
            };
            ko.bindingHandlers.event.init(element, newValueAccessor,
            allBindingsAccessor, viewModel, bindingContext);
        },
        update: function (element, valueAccessor, allBindingsAccessor,
        viewModel, bindingContext) {
            if ($(element).val() == ko.unwrap(valueAccessor())) {
                setTimeout(function () {
                    // Untoggle all buttons in the group
                    $(element).closest('.btn-group').children('label').removeClass('active');

                    // Now select the new active button
                    $(element).closest('label').addClass('active');
                }, 1);
            }
        }
    }

})