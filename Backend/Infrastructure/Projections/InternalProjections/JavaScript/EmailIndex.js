options({
    resultStreamName: 'user-email-index-res',
    reorderEvents: false,
    processingLag: 0
});

fromAll()
    .when({
        $init: function () {
            return {};
        },
        UserSignedUp: function (state, event) {
            const { data } = event;
            const eventData = { 
                Email: data.Email?.toLowerCase(),
                UserId: data.Id
            }

            emit('user-email-index-res', 'UserEmailIndexed', eventData);
        },
        UserDeleted: function (state, event) {
            const Email = JSON.parse(event.bodyRaw).Email?.toLowerCase();
            emit('user-email-index-res', 'UserEmailRemoved', { Email });
        },
    });
