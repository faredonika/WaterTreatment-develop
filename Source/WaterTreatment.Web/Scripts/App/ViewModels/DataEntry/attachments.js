'use strict';

define(['jquery', 'knockout', 'App/webService'], function ($, ko, webService) {
    return function (report) {
        var self = this,
            csrf = $('input[name="__RequestVerificationToken"]').val(),
            routePrefix = '/DataEntry/Attachments/';

        self.list = ko.observableArray();

        self.feedback = ko.observableArray();
        self.hideFeedback = function (fb) {
            self.feedback.remove(fb);
        };

        self.isUploading = ko.observable(false);
        self.attachLabel = ko.computed(function () {
            if (self.isUploading()) {
                return 'Uploading';
            }
            return 'Attach File';
        })
        self.fileSelected = ko.observable();
        self.updating = ko.observable(0);

        self.isUpdating = function (attachment) {
            return ko.computed(function () {
                var file = self.fileSelected();

                return (self.updating() === attachment.Id) && (file !== undefined && file !== null && file.length !== 0);
            });
        };

        var add = function (filename, payload) {
            var list = self.list();

            for (var i = 0; i < list.length; i++) {
                if (list[i].Name == filename) {
                    self.feedback.push({ css: 'alert-danger', message: 'Cannot upload file with duplicate name \'' + filename + '\'.' });
                    self.fileSelected('');
                    return;
                }
            }

            self.isUploading(true);
            webService.PostFile(routePrefix + 'Add/' + report.Id, payload).then(
                function (newAttachment) {

                    if (newAttachment.hasOwnProperty('Error')) {
                        self.feedback.push({ css: 'alert-danger', message: newAttachment.Error });
                        self.fileSelected('');
                    }
                    else {
                        self.list.push(newAttachment);
                        self.fileSelected('');
                    }
                },
                function (error) {
                    self.feedback.push({ css: 'alert-danger', message: 'Failed to upload attachment "' + filename + '".' });
                    self.fileSelected('');
                }
            ).then(function () {
                self.isUploading(false);
            });
        };

        var update = function (id, filename, payload) {
            webService.PostFile(routePrefix + 'Update/' + id, payload).then(
                function (result) {
                    if (result.hasOwnProperty('Error')) {
                        self.feedback.push({ css: 'alert-danger', message: result.Error });
                    } else {
                        self.feedback.push({ css: 'alert-success', message: 'Successfully updated file "' + result.Name + '".' });
                    }

                    self.fileSelected('');
                },
                function (error) {
                    self.feedback.push({ css: 'alert-danger', message: 'Failed to update attachment "' + filename + '".' });
                    self.fileSelected('');
                }
            ).then(function () {
                self.updating(0);
            });
        };

        self.fileSelected.subscribe(function (file) {
            if (file !== undefined && file !== null && file.length === 0)
                return;

            var filename = file.split('\\').pop(),
                payload = new FormData($('#uploadForm').get(0)),
                attachmentId = self.updating();

            payload.append('__RequestVerificationToken', csrf);

            if (attachmentId === 0) {
                add(filename, payload);
            } else {
                update(attachmentId, filename, payload);
            }
        });

        self.add = function () { $('#file').click(); };
        self.update = function (attachment) {
            self.updating(attachment.Id);
            $('#file').click();
        };

        self.remove = function (attachment) {
            $.post(routePrefix + 'Remove/' + attachment.Id, { __RequestVerificationToken: csrf })
                .done(function () {
                    self.list.remove(attachment);
                })
                .fail(function () {
                    self.feedback.push({ css: 'alert-danger', message: 'Failed to remove attachment "' + attachment.Name + '".' });
                });
        };

        webService.Get(routePrefix + report.Id).then(function (attachments) {
            self.list(attachments);
        });
    };
});