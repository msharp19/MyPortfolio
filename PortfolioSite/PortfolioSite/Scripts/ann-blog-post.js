$("#run-model").on("click", function () {
    var modelRunning = false;
    $(".output-text").text("");
    if (modelRunning === false) {
        modelRunning = true;
        $.ajax({
            type: "POST",
            url: "../Blog/RunModel",
            data: { connectionId: connectionId },
            success: function (data) {
                if (data.Status === "Success") {
                    outputSuccessMessage(data.Message, "Running");
                }
                else {
                    outputErrorMessage(data.Message, "Failure");
                }
                modelRunning = false;
                toggle();
            },
            error: function () {
                outputErrorMessage("Something went wrong :/", "Opps");
                modelRunning = false;
                toggle();
            }
        });
    }
});

function toggle() {

}