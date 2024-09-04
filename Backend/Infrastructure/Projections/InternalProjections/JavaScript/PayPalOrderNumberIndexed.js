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
        ExpenseCreated: function (state, event) {
            const { data } = event;

            const orderId = data?.Payment?.Response?.id;

            if (!orderId?.length) {
                return;
            }

            const eventData = { 
                IndexedValue: orderId,
                OwnerId: data.Id
            }

            emit('group-code-index-res', 'PayPalOrderNumberIndexed', eventData);
        },
    });
