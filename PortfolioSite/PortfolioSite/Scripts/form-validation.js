$("#comments").on("keyup", function () {
    checkField("#comments");
});

$("#email-val").on("keyup", function () {
    checkField("#email-val");
});

$("#name-val").on("keyup", function () {
    checkField("#name-val");
});

function checkField(id) {
    var fieldVal = $(id).val();
    var emptyField = false;
    var parentContainer = $(id).parent().find(".validation-email");
    if ((fieldVal && fieldVal.trim().length > 0)) {
        showSuccessMarker(parentContainer);
    } else {
        showErrorMarker(parentContainer);
        emptyField = true;
    }
    return emptyField;
}

function checkForEmptyFields() {
    var emptyField = false;
    var parentContainer = null;
    //Make the checks
    var comment = checkField("#comments");
    var email = checkField("#email-val");
    var name = checkField("#name-val");
    emptyField = emptyField === true ? true : comment;
    emptyField = emptyField === true ? true : email;
    emptyField = emptyField === true ? true : name;
    return !emptyField;
}

function showSuccessMarker(parentContainer) {
    parentContainer.find(".fa-check").show();
    parentContainer.find(".fa-exclamation").hide();
}

function showErrorMarker(parentContainer) {
    parentContainer.find(".fa-check").hide();
    parentContainer.find(".fa-exclamation").show();
}