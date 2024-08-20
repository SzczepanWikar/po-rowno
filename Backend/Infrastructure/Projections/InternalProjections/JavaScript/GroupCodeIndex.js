options({
    resultStreamName: 'group-code-index-res',
    reorderEvents: false,
    processingLag: 0
});

fromAll()
    .when({
        $init: function () {
            return {};
        },
        GroupCodeGenerated: function (state, event) {
            const { data } = event;
            const eventData = { 
                IndexedValue: data.Email?.toLowerCase(),
                OwnerId: data.Id
            }

            emit('group-code-index-res', 'GroupCodeIndexed', eventData);
        },
    });
