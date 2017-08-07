Select * From cfg.ContentTypes Where Name like '%javascript%'
Update cfg.ContentTypes Set Name = 'text/javascript' Where ID = 899

Select * From cfg.AllowedDataTypes 
Insert cfg.AllowedDataTypes Values(899, 4, '.js', NULL)

Select * From cfg.ContentTypes Where Name like '%json%'
Update cfg.ContentTypes Set Name = 'application/json' Where ID = 59

Select * From cfg.AllowedDataTypes 
Insert cfg.AllowedDataTypes Values(59, 4, '.js', NULL)