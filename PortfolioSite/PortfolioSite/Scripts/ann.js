$(function () {
    // Reference the auto-generated proxy for the hub.
    var progress = $.connection.annHub;
    var outputMessage = "</br>";
    var batch = 0;

    progress.client.addProgress = function (message) {
        outputMessage += message + "</br>";
        batch++;
        if (batch == 9) {
            batch = 0;
            $(".output-text").append(outputMessage);
        }
    };

    //Before doing anything with our hub we must start it
    $.connection.hub.start().done(function () {
        connectionId = $.connection.hub.id;
    });
});