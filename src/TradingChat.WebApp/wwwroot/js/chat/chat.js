
$(function(){
    scrollMessagesDown();

    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/Chat/Hub")
        .withAutomaticReconnect()
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

        scrollMessagesDown();
    });

    function scrollMessagesDown() {
        $(".chat-messages").scrollTop($(".chat-messages")[0].scrollHeight);
    }
    
    connection.start().then(function () {
        let chatId = $("#chat-id").val();
        connection.invoke("JoinChat", chatId)
            .catch(function (err) {
                return console.error(err.toString());
            });
    });

    $(".send-message-form").submit(function (e) {
        e.preventDefault();

        var formData = {
            ChatRoomId: $("#chat-id").val(),
            Message: $("#message-input").val(),
        }

        $.ajax({
            type: 'POST',
            url: '/Chat/SendMessage/', // Change this to your actual endpoint
            data: formData,
            success: function(response) {
                $("#message-input").val("");
            },
            error: function(xhr, status, error) {
                console.error(error.ToString())
            }
        });

        connection.invoke("SendMessage", {
            chatRoomId,
            message
        }).catch((err) => console.error(err.ToString()));
    });
})
