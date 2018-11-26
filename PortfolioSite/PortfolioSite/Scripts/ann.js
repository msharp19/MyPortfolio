$(function () {
    // Reference the auto-generated proxy for the hub.
    var progress = $.connection.annHub;

    progress.client.addProgress = function (message, limit) {
        outputMessage += message + "</br>";
        batch++;
        if (batch == 1000 || limit === false) {
            batch = 0;
            $(".output-text").append(outputMessage);
            outputMessage = "";
            $('#output').scrollTop($('.output-text').height());
        }
    };

    //Before doing anything with our hub we must start it
    $.connection.hub.start().done(function () {
        connectionId = $.connection.hub.id;
    });

});