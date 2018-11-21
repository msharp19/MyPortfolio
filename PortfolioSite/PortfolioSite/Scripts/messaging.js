function outputWarningMessage(errMsg, title) {
    Command: toastr["warning"](errMsg, title)
}

function outputSuccessMessage(successMsg, title) {
    Command: toastr["success"](successMsg, title)
}

function outputErrorMessage(errMsg, title) {
    Command: toastr["error"](errMsg, title)
}
