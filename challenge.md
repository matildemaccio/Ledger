# Let's Build a Ledger

Conduit asks that every candidate for an engineering role complete a take-home
exercise as a way of preparing for the technical interview. When you speak with
our engineering team as part of the technical interview process, you will be
asked to talk about your take-home exercise and to extend the code you wrote.

Before you begin, please read the exercise and instructions in full. Doing so
will help you understand what we're looking for, how to submit your code, and
prepare you for success in the next steps of the process! Pay special attention
to the instructions section at the end of this document.

## Overview

One of the fundamental building blocks of every financial application is a
ledger to keep track of money movements. Ledgers can be found everywhere,
powering all accounting software, blockchains and anything that interacts with
funds. Today we are going to be building an ledger and an API for that ledger.
We'll be using the ledger and API as the starting point for future interviews,
so make sure it is easy to run and modify.

## Our Ledger

The ledger we will be creating today is going to be a traditional double-entry
accounting ledger consisting of three primary objects: accounts, transactions,
and entries. Each of these objects is described in detail below. First though,
an introduction to double-entry accounting.

Double-entry accounting dates back to at least the 13th century, when it's known
to have been used by businesses in what is now Italy, although it could possibly
be significantly older. The core principle is that each transaction is recorded
in two entries - a debit, and a credit. Every credit must have a corresponding
equal-value debit, and every debit must likewise have a credit.

In order to debit (withdraw) funds from one account, those funds must be
credited (deposited) to another account. This allows easy detection of common
types of accounting errors - the sum of all credits and debits across all
accounts must always be 0, or some mistake has been made. (Note that a correct
sum does not guarantee that no mistakes have been made!)

A basic transaction representing the withdrawal of $100.00 from a "Discrtionary
Funds" account as cash to be placed in the owner's wallet might be represented
in a double-entry ledger system something like this:

| Discretionary Funds |    Cash      |
|---------------------|--------------|
| - 100.00            | + $100.00    |

Our ledger system also includes the notion of account directions - some accounts
represent primarily liabilities or the movement of funds out of our ownership,
while others track assets or incoming funds. This means we'll need to pay
careful attention to the relative direction of each entry: An entry representing
the removal of funds from our system is generally a debit, but when applied to
a debit account it should be expressed as a credit!

### Accounts

An **account** represents what a traditional account would in a double-entry
accounting system. It can be used to represent an asset, liability, expense or
anything else that we want. Some important properties that accounts have:

 - Accounts have a direction, either “debit” or “credit”.
 - Account balances can never be modified directly, they can only be modified
   by creating transactions.

### Transactions

A **transaction** represents an action which modifies the balances of the
accounts. It can be used to represent a purchase, paying a bill, paying
interest, moving money between accounts, etc. Transactions have a list of
entries, each of which represents modifications to the balance of an account.

Some important properties that transactions have:

 - The entries have to balance. This means the sum of all the debits must equal
   the sum of all the credits.

### Entries

An **entry** denotes a change in the balance of an account.

Here's the schema for an entry:
| Field     | Description                                               |
|-----------|-----------------------------------------------------------|
| id        | uuid (if not provided it is generated on object creation) |
| direction | string (one of "debit" or "credit")                       |
| amount    | int (representing amount in USD cents)                    |

## Rules

When users interact with the ledger we need to preserve some rules.

### Applying a Transaction

When a transaction is applied to the ledger, all the affected accounts should
be updated with the corresponding ledger entry amounts.

### Applying a Ledger Entry

When an entry is applied to an account, the balance in the account is updated
based on the direction of the account and the direction of the entry. If the
direction of the entry is different than the direction of the account the
balance of the account is reduced by the amount of the entry (the entry is
_subtracted_ from the account). Otherwise the balance of the account is
increased by the amount of the entry (the entry is _added_ to the account).

#### Example

Here are some example entries and their impact on the accounts they're applied
to:

| Starting Account Balance | Account Direction | Entry Direction | Entry Amount | Ending Account Balance |
|--------------------------|-------------------|-----------------|--------------|------------------------|
| 0                        | debit             | debit           | 100          | 100                    |
| 0                        | credit            | credit          | 100          | 100                    |
| 100                      | debit             | credit          | 100          | 0                      |
| 100                      | credit            | debit           | 100          | 0                      |

## API Guide

Users will need to interact with the ledger in order to see their balances and
create transactions. They'll do so using the HTTP/JSON API defined here.

### POST /account

| Field     | Description                                               |
|-----------|-----------------------------------------------------------|
| id        | uuid (if not provided it is generated on object creation) |
| name      | string (optional)                                         |
| balance   | integer (represents amounts in USD cents)                 |
| direction | string (one of "debit" or "credit")                       |

**Example Request:**

```bash
curl --request POST \
     --url https://localhost:5000/account \
     --header 'Accept: application/json' \
     --header 'Content-Type: application/json' \
     --data '
{
  "name": "test3",
  "direction": "debit",
  "id": "71cde2aa-b9bc-496a-a6f1-34964d05e6fd"
}
'
```

**Example Response:**

```json
{
  "balance": 0,
  "direction": "debit",
  "id": "71cde2aa-b9bc-496a-a6f1-34964d05e6fd",
  "name": "test3"
}
```

### GET /account/:id

```bash
curl --location --request GET 'localhost:5000/account/fa967ec9-5be2-4c26-a874-7eeeabfc6da8'
```

**Example Response:**

```json
{
  "balance": 0,
  "direction": "debit",
  "id": "71cde2aa-b9bc-496a-a6f1-34964d05e6fd",
  "name": "test3"
}
```

### POST /transactions

| Field   | Description                                               |
|---------|-----------------------------------------------------------|
| id      | uuid (if not provided it is generated on object creation) |
| name    | string (optional)                                         |
| entries | an array of ledger entry objects                          |

**Example Request:**

```bash
curl --location --request POST 'localhost:5000/transactions' \
     --header 'Content-Type: application/json' \
     --data-raw '
{
  "name": "test",
    "id": "3256dc3c-7b18-4a21-95c6-146747cf2971",
  "entries": [
    {
      "direction": "debit",
      "account_id": "fa967ec9-5be2-4c26-a874-7eeeabfc6da8",
      "amount": 100
    },
     {
      "direction": "credit",
      "account_id": "dbf17d00-8701-4c4e-9fc5-6ae33c324309",
      "amount": 100
    }
  ]
}'
```

**Example Response:**

```json
{
  "id": "3256dc3c-7b18-4a21-95c6-146747cf2971",
  "name": "test",
  "entries": [
    {
      "account_id": "fa967ec9-5be2-4c26-a874-7eeeabfc6da8",
      "amount": 100,
      "direction": "debit",
      "id": "9f694f8c-9c4c-44cf-9ca9-0cb1a318f0a7"
    },
    {
      "account_id": "dbf17d00-8701-4c4e-9fc5-6ae33c324309",
      "amount": 100,
      "direction": "credit",
      "id": "a5c1b7f0-e52e-4ab6-8f31-c380c2223efa"
    }
  ]
}
```

#### Notes about transactions

 - In the above example assuming both accounts exist and both have a debit
   direction , `fa967ec9-5be2-4c26-a874-7eeeabfc6da8` will have a balance of
   100 and `dbf17d00-8701-4c4e-9fc5-6ae33c324309` will have a balance of -100.

## Instructions

Implement the ledger described above. Your ledger should provide the API
described, accessible via HTTP. You can use any programming language or
framework you like. Additionally, don't worry about recording ledger entries
in a database - we're interested in your business logic and your API, and will
not be looking at what storage technology was used.

Note: Ledgers can seem deceptively simple. We encourage you to write tests to
verify that you've covered all of the edge cases!

Once you've finished implementing the ledger as directed, package up your work
in either a compressed tarball (.tar.gz, .tar.bz2) or a zip file (.zip). Make
sure you include clear instructions on how to run your code (eg, a README.md).
Pay special attention to any environmental requirements or dependencies your
code might have so that we can configure things correctly.

You should have been provided an upload URL by email as part of the exercise.
Upload the file containing your work to that URL. If you did not receive a URL,
or if you need to re-upload your exercise, please reach out to
careers@conduit.financial for assistance.

If you run into trouble, or have any questions, please don't hesitate to reach
out to our team for help. You can reach us by replying to the email that you
received the exercise from, or by reaching out to carreers@conduit.financial.

