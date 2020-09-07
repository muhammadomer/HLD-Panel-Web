$ ->
	window.Background = 
		
		internal:
			_document : $(document)
			_window : $(window)
			_body : $('body')
		
		tick:(options)->
			# internal params
			params = this.internal

			# default set
			constructor =
				source : {}
				duration : "1s"
				easeType : "ease-out"
				callback : ()->

			# alternative arguments form
			if arguments.length
				if options.imageUrl or options.json or options.jsonString
					options = 
						source : arguments[0]
						callback : arguments[1]
			options = $.extend constructor, options
			
			# initialize global variable
			dom_document = document
			regExp_url = /^(?:([A-Za-z]+):)?(\/{0,3})([0-9.\-A-Za-z]+)(?::(\d+))?(?:\/([^?#]*))?(?:\?([^#]*))?(?:#(.*))?$/

			# change background-image as json defined
			change = (value)->
				if typeof value is "string" then value = $.parseJSON(value)
				day = value.backgrounds
				now = new Date()
				for moment in day
					start = moment.started_at.split(":")
					end = moment.ended_at.split(':')
					if start[0] is end[0]
						if now.getHours() is parseInt(start[0]) and start[1]<=now.getMinutes()<=end[1] then yes
						else continue
					else
						if parseInt(start[0])<now.getHours()<parseInt(end[0]) then yes
						else if now.getHours() is parseInt(start[0]) and now.getMinutes() >= parseInt(start[1]) then yes
						else if now.getHours() is parseInt(end[0]) and now.getMinutes() <= parseInt(end[1]) then yes
						else continue
					this_moment = moment
				if this_moment
					if this_moment.image
						if typeof jQuery isnt 'undefined' then params._body.css("background-image","url(#{this_moment.image})")
						else
							if dom_document.styleSheets[0].insertRule then dom_document.styleSheets[0].insertRule("body {background-image:url(#{this_moment.image});}",0)
							else if dom_document.styleSheets[0].addRule then dom_document.styleSheets[0].addRule("body","background-image:url(#{this_moment.image})",0)
					if this_moment.color then params._body.css("background-color",this_moment.color)

			# deal with different source		
			if options.source.json then change options.source.json
			else if options.source.jsonString
				if regExp_url.test(options.source.jsonString)
					$.ajax({
						url : options.source.jsonString
						data : {}
						success : (value)->
							change value
					})
				else change options.source.jsonString
			else if options.source.imageUrl
				if typeof jQuery isnt 'undefined' then params._body.css("background-image","url(#{options.source.imageUrl})")
				else
					if dom_document.styleSheets[0].insertRule then dom_document.styleSheets[0].insertRule("body {background-image:url(#{options.source.imageUrl});}",0)
					else if dom_document.styleSheets[0].addRule then dom_document.styleSheets[0].addRule("body","background-image:url(#{options.source.imageUrl})",0)
			
			# initialize css
			animation_style = "background-position #{options.duration} #{options.easeType}"
			params._body.css({'transition':animation_style, 'moz-transition':animation_style, '-webkit-transition':animation_style, '-o-transition':animation_style, '-ms-transition':animation_style, 'background-repeat':'no-repeat', 'background-attachment':'fixed'})
			
			return true if not params._body.css('background-position')
			coordinate = params._body.css('background-position').split(' ')
			[document_height, window_height] = [params._document.height(), params._window.height()]
			
			# define callback
			params._body.on 'transitionend webkitTransitionEnd oTransitionEnd otransitionend',(event)->
				options.callback()
			
			# define resize event
			params._window.on 'resize', (event)->
				window_height = $(this).height()
			
			# background slide with window scroll
			params._window.on 'scroll', (event)->
				document_height = if document_height is params._document.height() then document_height else params._document.height()
				scroll_height = params._window.scrollTop()
				image_scroll = (scroll_height/(document_height - window_height)*(100-parseInt(coordinate[1]))+parseInt(coordinate[1])).toFixed(7)
				params._body.css("background-position",coordinate[0]+" #{image_scroll}%")