define(['knockout', 'jquery', 'bootstrap'], function (ko, $) {

    var vm = function () {

        var self = this;

        self.textBinding = ko.observable();

        self.binding = ko.observable();

        self.display = function (textBinding) {
            self.textBinding(textBinding());
            self.binding = textBinding;
            $('#commentDialog').modal('show');
        };

        self.save = function () {
            self.binding(self.textBinding());
            $('#commentDialog').modal('hide');
        }

    };

    return new vm();

});