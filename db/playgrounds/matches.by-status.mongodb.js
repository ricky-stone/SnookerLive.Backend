use('Matches');

const query = {
  _id: "2346-8-1",
  EventId: 2346,
};

const count = db.getCollection('all').countDocuments(query);

const results = db.getCollection('all')
  .find(query)
  .toArray();

({ count, results });