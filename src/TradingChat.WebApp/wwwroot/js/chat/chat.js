
$(function(){
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/Chat/Hub")
        .build();

    connection.on("ReceiveMessage", function (message) {

        var messageHtml =
            '<div class="chat-message">' +
            '<div class="chat-message-header">' +
            '<span class="chat-message-sender">' + message.user + '</span>' +
            '<span class="chat-message-time">' + message.sentAtString + '</span>' +
            '</div>' +
            '<div class="chat-message-text">' + message.message + '</div>' +
            '</div>';

        $(".chat-messages").append(messageHtml);

        $(".chat-messages").scrollTop($(".chat-messages")[0].scrollHeight);
    });

    connection.start().then(function () {
        let chatId = $("#chat-id").val();
        connection.invoke("JoinChat", chatId)
            .catch(function (err) {
                return console.error(err.toString());
            });
    });

    $(".send-message-form").submit(function (e) {
        e.preventDefault();

        let chatRoomId = $("#chat-id").val();
        let message = $(".message-input").val();

        $(".message-input").val("");

        connection.invoke("SendMessage", {
            chatRoomId,
            message
        });
    });
})
