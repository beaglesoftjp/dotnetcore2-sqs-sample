# dotnetcore2-sqs-sample
このプロジェクトはASP.NET MVC core 2でSQSを処理するためのサンプルプロジェクトです。

## 前提条件
AWS SQSをaws cliから利用可能であることを前提とします。

## 実行方法
以下の通り実行することで動作を確認できます。

```bash
$ ASPNETCORE_ENVIRONMENT=Development dotnet run
```

### 1件のQueueメッセージを登録する
POSTのbodyにメッセージ(`value1`)を設定しSQSへ登録します。

```bash
$ curl -d '"value1"' -H "Content-type: application/json"  -XPOST localhost:5000/api/sqs/create
{
  "StatusCode": "正常終了",
  "Message": "SQSへメッセージの登録に成功しました。"
}
```

### 複数のQueueメッセージを登録する
POSTのbodyにメッセージ(`value1`, `value2`)を設定しSQSへ登録します。

```bash
$ curl -d '["value1","value2"]' -H "Content-type: application/json"  -XPOST localhost:5000/api/sqs/batchcreate
{
  "StatusCode": "正常終了",
  "Message": "SQSへメッセージの登録に成功しました。",
  "SuccessfulResponse": [
    {
      "Id": "0",
      "MD5OfMessageAttributes": null,
      "MD5OfMessageBody": "9946687e5fa0dab5993ededddb398d2e",
      "MessageId": "aee58b7f-3b15-4768-8f18-721b01f80f1d",
      "SequenceNumber": null
    },
    {
      "Id": "1",
      "MD5OfMessageAttributes": null,
      "MD5OfMessageBody": "f066ce9385512ee02afc6e14d627e9f2",
      "MessageId": "2894633a-67f6-4ab5-ae24-3343a79cd091",
      "SequenceNumber": null
    }
  ],
  "FailedResponse": []
}```

### メッセージを取得します
```bash
$ curl localhost:5000/api/sqs/receive

 {
    "StatusCode": "正常終了",
    "Message": "SQSへメッセージの受信に成功しました。",
    "QueueMessages": [
      {
        "Attributes": {},
        "Body": "value1",
        "MD5OfBody": "9946687e5fa0dab5993ededddb398d2e",
        "MD5OfMessageAttributes": null,
        "MessageAttributes": {},
        "MessageId": "2acb96f3-2d0d-4c4b-84b8-c16c4baf6be6",
        "ReceiptHandle": "AQEBQkAPB7e1A8gSsEh5OtL0Vxk0UmNzl2wUsddlBP04r+HXQ03/GFiZftgtkoPxwYW8Bhu+TwXrdGgo99TVgiTQq3zP5qRd/EPjY2nQlwHYpeeikRjlvu1yBF5+3mp6ZTS7Ff1LCoVp210akf0OvEl+S3YxJajjne3hNrcOt+tLdEdojPhQ7mp/dVFqsSVcfRn2b9zxqkPL105Rc9dEthBkv3VXvjYrnvJC5M1dsBpghXPpHpiuh+Kbtvs0CdxOjwIyLb/KzxBAAvLJoPjzm2se0hYaFLqeyonGxcSOm8eRfnz7+Rh/vyqu7QiT8dAGTWe+MBWa4Fx6qq9M4QBvMmCTyNQmlNel6+yHN3lHDjrnBCA9DlX9epn9GH1knBEYCxeDFrZAvT70/JPykneybJGKEQ=="
      }
    ]
  }
```

### メッセージを削除します
```bash
$ curl -X "DELETE" localhost:5000/api/sqs/delete
{
  "StatusCode": "正常終了",
  "Message": "SQSへメッセージの削除に成功しました。"
}
```

