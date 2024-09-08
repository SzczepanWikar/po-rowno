options({
    resultStreamName: 'user-code-index-res',
    reorderEvents: false,
    processingLag: 0
});

fromAll()
    .when({
        $init: function () {
            return {};
        },
        UserCodeGenerated: function (state, event) {
            const { data } = event;
            const eventData = {
                IndexedValue: data.Code.Value,
                OwnerId: data.Id
            }

            emit('user-code-index-res', 'UserCodeIndexed', eventData);
        },
    });
