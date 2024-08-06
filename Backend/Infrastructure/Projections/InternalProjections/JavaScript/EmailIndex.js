options({
    resultStreamName: 'user-email-index-res',
    reorderEvents: false,
});

fromAll()
    .when({
        $init: function () {
            return {};
        },
        UserSignedUp: function (state, event) {
            const Email = JSON.parse(event.bodyRaw).Email?.toLowerCase();
            emit('user-email-index-res', 'UserEmailIndexed', { Email });
        },
        UserDeleted: function (state, event) {
            const Email = JSON.parse(event.bodyRaw).Email?.toLowerCase();
            emit('user-email-index-res', 'UserEmailRemoved', { Email });
        },
    });
