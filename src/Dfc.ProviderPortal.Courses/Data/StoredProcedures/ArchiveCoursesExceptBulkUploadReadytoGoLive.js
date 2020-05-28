
function ArchiveCourses(UKPRN, statusToBeChangedTo) {
    var collection = getContext().getCollection();
    var collectionLink = collection.getSelfLink();
    var response = getContext().getResponse();
    var BulkUploadReadyToGoLiveCourseStatus = 32;
    var ArchivedStatus = 4;

    var responseBody = {
        updated: 0,
        continuation: true,
        error: "",
        log: ""
    };

    if (typeof statusToBeChangedTo !== 'number') throw new Error('statusToBeChangedTo must be a number');
    if (typeof UKPRN !== 'number') throw new Error('UKPRN must be a number');

    var updated = 0;
    tryQueryAndUpdate();
    function tryQueryAndUpdate(continuation) {
        var query = {
            query: "select * from courses c where c.CourseStatus != @BulkUploadReadyToGoLiveCourseStatus and c.CourseStatus != @ArchivedStatus and c.ProviderUKPRN = @UKPRN",
            parameters: [
                { name: "@archivedStatus", value: archivedStatus },
                { name: "@BulkUploadReadyToGoLiveCourseStatus", value: BulkUploadReadyToGoLiveCourseStatus },
                { name: "@UKPRN", value: UKPRN }]
        };

        var requestOptions = { continuation: continuation };
        var isAccepted = collection.queryDocuments(collectionLink, query, requestOptions, function (err, documents, responseOptions) {
            if (documents.length > 0) {
                for (var i = 0; i < documents.length; i++)
                    tryUpdate(documents[i]);
                tryQueryAndUpdate(responseOptions.continuation);
            } else if (responseOptions.continuation) {
                tryQueryAndUpdate(responseOptions.continuation);
            } else {
                response.setBody({ updated: updated });
            }
        });
        if (!isAccepted) {
            throw new Error("The stored procedure timed out.");
        }
    }

    function tryUpdate(document) {
        var requestOptions = { etag: document._etag };

        document.CourseStatus = statusToBeChangedTo;
        document.CourseRuns.forEach(l => l.RecordStatus = statusToBeChangedTo);

        var isAccepted = collection.replaceDocument(document._self, document, requestOptions, function (err, updatedDocument, responseOptions) {
            if (err) throw err;
            updated++;
        });
        if (!isAccepted) {
            throw new Error("The stored procedure timed out 2.");
        }
    }
}