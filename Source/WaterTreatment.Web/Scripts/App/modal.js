'use strict';
define(['jquery', 'knockout', 'dynamicHtml'], function ($, ko) {
    var vm = function (title, bodyHtml, proceedText, disableProceed, proceed, cancel, showProceed) {
        var self = this;
        var defaults = {
            title: title,
            bodyHtml: bodyHtml,
            proceedText: proceedText || 'OK',
            disableProceed: disableProceed != undefined ? disableProceed : false,
            showProceed: showProceed !== undefined ? showProceed : true,
            proceed: proceed || $.noop,
            cancel: cancel || $.noop
        };
        self.title = ko.observable(defaults.title);
        self.bodyHtml = ko.observable(defaults.bodyHtml);
        self.proceedText = ko.observable(defaults.proceedText);
        self.disableProceed = ko.observable(defaults.disableProceed);
        self.showProceed = ko.observable(defaults.showProceed);
        self.proceed = defaults.proceed;
        self.cancel = defaults.cancel;

        self.populate = function (t, bh, pt, dp, p, c, sp) {
            self.title(t || defaults.title);
            self.bodyHtml(bh || defaults.bodyHtml);
            self.proceedText(pt || defaults.proceedText);
            self.disableProceed(dp);
            self.showProceed(sp);
            self.proceed = p || defaults.proceed;
            self.cancel = c || defaults.cancel;
        };
    };
    return vm;
});