"use strict";

requirejs.config({
    baseUrl: "/Scripts",
    paths: {
        jquery: "jquery-2.1.4",
        knockout: "knockout-3.3.0.debug",
        koVal: "knockout.validation",
        webService: "App/webService",
        pager: "App/pager",
        notesBox: "App/notesbox",
        datePicker: "App/Bindings/datePicker",
        dynamicHtml: "App/Bindings/dynamicHtml",
        collapsed: "App/Bindings/collapsed",
        hasFocus: "App/Bindings/hasFocus",
        util: "App/utilities",
        modal: "App/modal",
        googleCharts: '//www.gstatic.com/charts/loader'
    },
    shim: {
        bootstrap: {
            deps: ["jquery"]
        }
    }
});