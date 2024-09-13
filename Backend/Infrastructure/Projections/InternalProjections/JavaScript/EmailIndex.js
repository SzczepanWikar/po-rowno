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
                IndexedValue: data.Email?.toLowerCase(),
                OwnerId: data.Id
            }

            emit('user-email-index-res', 'UserEmailIndexed', eventData);
        },
        AccountDeleted: function (state, event) {
            const { data } = event;

            const IndexedValue = data.Email?.toLowerCase();
            emit('user-email-index-res', 'UserEmailRemoved', { IndexedValue });
        },
    });
