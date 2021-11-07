# ResultSharp 

[![Build and Test](https://github.com/nekronos/ResultSharp/workflows/Build%20and%20Test/badge.svg)](https://github.com/nekronos/ResultSharp/actions)
[![NuGet](https://img.shields.io/nuget/v/ResultSharp.svg?color=mediumturquoise&logo=NuGet)](https://www.nuget.org/packages/ResultSharp/)

This library provides an implmentation of a Result monad. With its API inspired by [Rust's Result type](https://doc.rust-lang.org/std/result/enum.Result.html)
## Usage

```c#
using ResultSharp;
using static ResultSharp.Prelude;
```

## Examples

#### Construction
```c#
var ok = Result.Ok(); // Ok result without a value
var err = Result.Err("foobar"); // Faulted result with error message

var ok = Result.Ok<int>(100); // Ok result with a value
var err = Result.Err<int>("foobar");

var ok = Result.Ok<string, int>("foobar");
var err = Result.Err<string, int>(-1); // Faulted result with an int as error value
```

#### Match
```c#
var result = Result.Ok<string, int>("foobar");

// calls the func matching the result state
result.Match(
  ok: value => Console.WriteLine(value),
  err: error => Console.WriteLine(error)
);

```

#### Map
```c#
var result = Result
  .Ok()
  .Map(() => "foobar") // Unit Result is promoted to Result<string>
  .Map(value => value.Length);
```

#### MapErr
```c#
```

#### And
```c#
```

#### AndThen
```c#
```

#### Or
```c#
```

#### OrElse
```c#
```

#### BiMap
```c#
```

#### Combine
```c#
```

#### Unwrap
```c#
```

#### Expect
```c#
```

#### Equality
```c#
```

#### Linq
```c#
```