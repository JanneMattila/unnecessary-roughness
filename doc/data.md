# Data

## JSON payload examples

### Base event

All events contain following structure:

```json
{
  "id": "123-abc-123",
  "type": "EventType",
  "data" : {
    // Extension data
  }
}
```

## Table Storage structure

### Events table

Game data is stored as events. Each action in game is stored as single row.
Single game events are stored in single partition to enable fast retrieval
of game events.

| PartitionKey | RowKey | Data |
|--------------|--------|------|
| 123-abc-111  | 100001 | ...  |
| 123-abc-111  | 100002 | ...  |
| 123-abc-111  | 100003 | ...  |

`PartitionKey`: game unique identifier

`RowKey`: ordered number

`Data`: Event data
