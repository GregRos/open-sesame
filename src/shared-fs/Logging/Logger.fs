[<AutoOpen>]
module Shared.Logging

open Microsoft.Extensions.Logging
open Microsoft.Extensions.Logging.Console
open Microsoft.Extensions.Logging.Abstractions
open System.IO
open System

let minimumLevel = LogLevel.Trace

type MyConsoleFormatter2() =
    inherit ConsoleFormatter "Beautiful"

    override this.Write
        (logEntry: inref<LogEntry<'TState>>, scopeProvider: IExternalScopeProvider, textWriter: TextWriter)
        : unit =
        let dt = DateTime.Now
        let message = logEntry.State
        textWriter.WriteLine $"[{dt}] {message}"
        textWriter.Flush()


let initBuilder (builder: ILoggingBuilder) =
    builder.AddConsole().AddDebug().SetMinimumLevel minimumLevel |> ignore

    builder.AddConsoleFormatter<MyConsoleFormatter2, ConsoleFormatterOptions>()
    |> ignore


type ILogger with
    member this.error(message: string) = this.LogError(message)

    member this.info(message: string) = this.LogInformation(message)

    member this.debug(message: string) = this.LogDebug(message)

    member this.trace(message: string) = this.LogTrace(message)

    member this.warn(message: string) = this.LogWarning(message)

let Loggers =
    let factory = LoggerFactory.Create initBuilder
    factory
