options({
    resultStreamName: 'user-refresh-token-index-res',
    reorderEvents: false,
    processingLag: 0
});

fromAll()
    .when({
        $init: function () {
            return {};
        },
        UserSignedIn: function (state, event) {
            const { data } = event;
            const { RefreshToken } = data;


            if (!RefreshToken?.Token?.length) {
                return;
            }

            const eventData = {
                IndexedValue: RefreshToken.Token,
                OwnerId: data.Id
            }

            emit('user-refresh-token-index-res', 'UserRefreshTokenIndexed', eventData);
        },
    });
