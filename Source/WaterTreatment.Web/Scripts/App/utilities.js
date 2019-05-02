'use strict';
define([], function () {
    var util = {};

    util.promptForCommand = function(message, match, onMatched, onCanceled, feedbackMessage) {
        if (!match || match.trim() === '') throw 'Nothing to match against';

        var command = window.prompt((feedbackMessage || '') + message, '');
        if (command === null /* most browsers */ || command === '' /* Safari */ || command === undefined /* probably not possible with current browsers but just in case */) {
            if (onCanceled) onCanceled();
        } else if (command.trim().toLowerCase() === match.toLowerCase()) {
            if (onMatched) onMatched();
        } else {
           util. promptForCommand(message, match, onMatched, onCanceled, feedbackMessage || 'The text you entered does not match the requested text.\r\n');
        }
    };

    return util;
});