module EasterGiraffe.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

// ---------------------------------
// Models
// ---------------------------------

type Input = (int * DateTime * DateTime) list

// ---------------------------------
// Views
// ---------------------------------

module Views =
    open GiraffeViewEngine

    let layout (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "EasterGiraffe" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href "main.css" ]
            ]
            body [] content
        ]
    
    let encodeYear (x : int) = a [ _href (String.Format ("/{0}", x)) ] [ x.ToString() |> encodedText ]

    let encodeDay (x : DateTime) = x.ToString("MMMM dd") |> encodedText

    let encodeHeader (x : string) = th [] [ encodedText x ]

    let yearView data =
        let (year, catholic, orthodox) = data
        if catholic = orthodox then
            tr [] [ td [] [ encodeYear year ]; td [ _colspan "2" ] [ encodeDay catholic ] ] else
            tr [] [ td [] [ encodeYear year ]; td [] [ encodeDay catholic ]; td [] [ encodeDay orthodox ] ]

    let index (model : Input) =
        [
            h1 [] [ encodedText "Nearest Easter dates" ]
            table [] [
                List.map encodeHeader [ "Year"; "Catholic Easter"; "Orthodox Easter" ] |> thead []
                List.map yearView model |> tbody [] 
            ]
        ] |> layout

// ---------------------------------
// Web app
// ---------------------------------

let generateData year = 
    let gauss = Easter.gaussAlgorithm year
    (year, gauss Denomination.Catholic, gauss Denomination.Orthodox)

let indexHandler (year : int) =
    if year < 11 then
        setStatusCode 400 >=> text "Year too small" 
    else if year > 9989 then 
        setStatusCode 400 >=> text "Year too big" 
    else
        let model     = List.map generateData [ year - 10 .. year + 10 ]
        let view      = Views.index model
        htmlView view

let webApp =
    choose [
        GET >=>
            choose [
                route "/" >=> indexHandler DateTime.UtcNow.Year
                routef "/%i" indexHandler
            ]
        setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080")
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IHostingEnvironment>()
    (match env.IsDevelopment() with
    | true  -> app.UseDeveloperExceptionPage()
    | false -> app.UseGiraffeErrorHandler errorHandler)
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) = l.Equals LogLevel.Error
    builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main _ =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(contentRoot)
        .UseIISIntegration()
        .UseWebRoot(webRoot)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0
