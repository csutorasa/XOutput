import React from "react";
import CircularProgress from '@material-ui/core/CircularProgress';
import Typography from '@material-ui/core/Typography';

export interface AsyncComponentProps<T> {
    task: Promise<T>;
    render: (data?: T) => any;
}

interface AsyncComponentState<T> {
    loaded: boolean;
    success?: boolean;
    result?: T;
    error?: string;
}

export function AsyncErrorHandler(component: React.Component): (err: any) => void {
    console.log('use')
    return (err: any) => {
        console.error('Error happened', err);
        component.forceUpdate();
        throw err;
    }
}

class AsyncComponent<T> extends React.Component<AsyncComponentProps<T>, AsyncComponentState<T>> {

    state: AsyncComponentState<T> = {
        loaded: false,
    }
    private subscribed: boolean = false;

    componentDidMount() {
        if (this.props.task) {
            this.subscribe();
        }
    }

    componentDidUpdate() {
        if (!this.subscribed && this.props.task) {
            this.subscribe();
        }
    }

    private subscribe(): void {
        this.props.task.then((data) => {
            this.setState({
                loaded: true,
                success: true,
                result: data,
                error: null,
            })
        }, (err: Error) => {
            this.setState({
                loaded: true,
                success: false,
                result: null,
                error: err.toString(),
            })
            console.log('' + err)
        });
        this.subscribed = true;
    }

    render() {
        if (this.state.loaded === false) {
            return <div><CircularProgress /></div>
        }
        if (this.state.success === false) {
            return <Typography color='error'>{this.state.error}</Typography>
        }
        return <>
            {this.props.render(this.state.result)}
        </>;
    }
}

export const Async = AsyncComponent;
export default Async;
