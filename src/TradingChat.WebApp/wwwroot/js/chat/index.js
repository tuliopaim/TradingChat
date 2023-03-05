$(function () {
    $(".join-chat-form").submit(function (e) {
        e.preventDefault();

        var form = $(this);

        var request = $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
        });

        var summary = $("#validation-summary > ul");

        request.then(handleSuccess)
            .catch(function (xhr) { handleBadRequest(xhr, summary); })
            .catch(function () { handleOtherError(summary); })
            .catch(function () { handleNetworkError(summary); });
    });
});

function handleSuccess(response) {
    window.location.href = "/Chat/" + response;
}

function handleBadRequest(xhr, summary) {
    if (xhr.status === 400) {
        showError(xhr.responseJSON.errors, summary);
    } else {
        showError(["An error occurred while joining the chat."]);
    }
}

function handleOtherError(summary) {
    showError(["An error occurred while joining the chat."], summary);
}

function handleNetworkError(summary) {
    summary.html("An error occurred while joining the chat.", summary);
}

function showError(errors, summary) {
    summary.fadeOut(200, function () {

        summary.empty();

        errors.forEach(function (error) {
            summary.append($('<li>').text(error));
        });

        summary.fadeIn(400);
    });

}