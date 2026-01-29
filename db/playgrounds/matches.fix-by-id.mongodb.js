use('Matches');

const id = "2346-9-2";

const updates = {
  Round: 99
};

db.getCollection('all').findOneAndUpdate(
  { _id: id },
  { $set: updates },
  { returnDocument: "after" }
);